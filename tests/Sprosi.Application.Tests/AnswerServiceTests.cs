using Sprosi.Application.Answers;
using Sprosi.Application.Common;
using Sprosi.Application.Questions;
using Sprosi.Application.Tests.Fakes;
using Sprosi.Domain;

namespace Sprosi.Application.Tests;

public sealed class AnswerServiceTests
{
    private readonly FakeQuestionRepository _questions = new();
    private readonly FakeClock _clock = new();
    private readonly Guid _askerId = Guid.NewGuid();
    private readonly Guid _helperId = Guid.NewGuid();

    private QuestionService Questions => new(_questions, _clock);

    private AnswerService Answers => new(new FakeAnswerRepository(_questions), _questions, Questions, _clock);

    [Fact]
    public async Task Update_rejects_foreign_answer()
    {
        var question = await Ask("Как доехать?");
        var answered = await Answers.CreateAsync(question.Id, _helperId, "На трамвае.", CancellationToken.None);
        var answerId = answered.Answers.Single().Id;

        var error = await Assert.ThrowsAsync<ForbiddenException>(() =>
            Answers.UpdateAsync(answerId, _askerId, "Другой текст", CancellationToken.None));

        Assert.Equal("Нельзя изменить чужой ответ.", error.Message);
    }

    [Fact]
    public async Task Accept_rejects_own_answer()
    {
        var question = await Ask("Что почитать?");
        var answered = await Answers.CreateAsync(question.Id, _askerId, "Сам пока не знаю.", CancellationToken.None);

        var error = await Assert.ThrowsAsync<ValidationException>(() =>
            Answers.AcceptAsync(answered.Answers.Single().Id, _askerId, CancellationToken.None));

        Assert.Equal("Нельзя отметить свой ответ как лучший.", error.Message);
        Assert.False(answered.Answers.Single().IsAccepted);
    }

    [Fact]
    public async Task Accept_replaces_the_previous_best_answer()
    {
        var question = await Ask("Где поесть?");
        var first = await Answers.CreateAsync(question.Id, _helperId, "В столовой.", CancellationToken.None);
        var secondAuthor = Guid.NewGuid();
        var both = await Answers.CreateAsync(question.Id, secondAuthor, "На кухне.", CancellationToken.None);
        var firstId = both.Answers.Single(answer => answer.Body == "В столовой.").Id;
        var secondId = both.Answers.Single(answer => answer.Body == "На кухне.").Id;

        var acceptedFirst = await Answers.AcceptAsync(firstId, _askerId, CancellationToken.None);
        var acceptedSecond = await Answers.AcceptAsync(secondId, _askerId, CancellationToken.None);

        Assert.True(acceptedFirst.HasAcceptedAnswer);
        Assert.True(acceptedSecond.Answers.Single(answer => answer.Id == secondId).IsAccepted);
        Assert.False(acceptedSecond.Answers.Single(answer => answer.Id == firstId).IsAccepted);
        Assert.Equal(first.Id, question.Id);
    }

    [Fact]
    public async Task Accept_rejects_someone_who_did_not_ask()
    {
        var question = await Ask("Какой язык выбрать?");
        var answered = await Answers.CreateAsync(question.Id, _helperId, "Тот, на котором уже пишете.", CancellationToken.None);

        var error = await Assert.ThrowsAsync<ForbiddenException>(() =>
            Answers.AcceptAsync(answered.Answers.Single().Id, _helperId, CancellationToken.None));

        Assert.Equal("Отметить ответ может только автор вопроса.", error.Message);
    }

    private async Task<QuestionDetails> Ask(string title)
    {
        return await Questions.CreateAsync(
            _askerId,
            new CreateQuestionRequest(title, "Нужен короткий ответ.", "Study"),
            CancellationToken.None);
    }
}
