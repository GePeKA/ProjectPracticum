using Sprosi.Application.Common;
using Sprosi.Application.Questions;
using Sprosi.Application.Tests.Fakes;
using Sprosi.Domain;

namespace Sprosi.Application.Tests;

public sealed class QuestionServiceTests
{
    private readonly FakeQuestionRepository _questions = new();
    private readonly FakeClock _clock = new();
    private readonly Guid _authorId = Guid.NewGuid();

    private QuestionService Service => new(_questions, _clock);

    [Fact]
    public async Task Create_rejects_blank_title()
    {
        var error = await Assert.ThrowsAsync<ValidationException>(() =>
            Service.CreateAsync(_authorId, new CreateQuestionRequest("  ", "Текст", "Study"), CancellationToken.None));

        Assert.Equal("Заполните заголовок и текст вопроса.", error.Message);
        Assert.Empty(_questions.Questions);
    }

    [Fact]
    public async Task Update_rejects_foreign_author()
    {
        var created = await Service.CreateAsync(
            _authorId,
            new CreateQuestionRequest("Как варить рис?", "Нужен совет.", "Everyday"),
            CancellationToken.None);

        var error = await Assert.ThrowsAsync<ForbiddenException>(() =>
            Service.UpdateAsync(
                created.Id,
                Guid.NewGuid(),
                new UpdateQuestionRequest("Другой заголовок", "Другой текст", "City"),
                CancellationToken.None));

        Assert.Equal("Нельзя изменить чужой вопрос.", error.Message);
        Assert.Equal("Как варить рис?", _questions.Questions[0].Title);
    }

    [Fact]
    public async Task Get_allows_edit_only_for_author()
    {
        var created = await Service.CreateAsync(
            _authorId,
            new CreateQuestionRequest("Куда сходить?", "Ищу короткую прогулку.", "City"),
            CancellationToken.None);

        var asAuthor = await Service.GetAsync(created.Id, _authorId, CancellationToken.None);
        var asGuest = await Service.GetAsync(created.Id, null, CancellationToken.None);

        Assert.True(asAuthor.CanEdit);
        Assert.False(asGuest.CanEdit);
    }

    [Fact]
    public async Task List_reports_answer_count_and_clamps_page_size()
    {
        var created = await Service.CreateAsync(
            _authorId,
            new CreateQuestionRequest("Где взять конспект?", "Нужен короткий.", "Study"),
            CancellationToken.None);

        var stored = _questions.Questions.Single(question => question.Id == created.Id);
        stored.Answers.Add(new Answer
        {
            Id = Guid.NewGuid(),
            QuestionId = stored.Id,
            Body = "Вот он.",
            IsAccepted = true,
        });

        var page = await Service.ListAsync(null, "resolved", "popular", null, 0, 500, CancellationToken.None);

        Assert.Equal(1, _questions.LastQuery!.Page);
        Assert.Equal(50, _questions.LastQuery.PageSize);
        Assert.Equal(QuestionStatusFilter.Resolved, _questions.LastQuery.Status);
        Assert.Equal(QuestionSort.Popular, _questions.LastQuery.Sort);
        var item = Assert.Single(page.Items);
        Assert.Equal(1, item.AnswerCount);
        Assert.True(item.HasAcceptedAnswer);
    }
}
