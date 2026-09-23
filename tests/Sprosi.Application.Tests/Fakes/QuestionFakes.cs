using Sprosi.Application.Abstractions;
using Sprosi.Application.Questions;
using Sprosi.Domain;

namespace Sprosi.Application.Tests.Fakes;

internal sealed class FakeQuestionRepository : IQuestionRepository
{
    public List<Question> Questions { get; } = [];

    public QuestionListQuery? LastQuery { get; private set; }

    public Task<Question?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Questions.FirstOrDefault(question => question.Id == id));

    public Task AddAsync(Question question, CancellationToken cancellationToken)
    {
        question.Author ??= new User { Id = question.AuthorId, DisplayName = "Автор" };
        Questions.Add(question);
        return Task.CompletedTask;
    }

    public Task SaveAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task RemoveAsync(Question question, CancellationToken cancellationToken)
    {
        Questions.Remove(question);
        return Task.CompletedTask;
    }

    public Task<QuestionPage> ListAsync(QuestionListQuery query, CancellationToken cancellationToken)
    {
        LastQuery = query;
        var items = Questions
            .Where(question => query.Topic is null || question.Topic == query.Topic)
            .Select(question => new QuestionListItem(
                question.Id,
                question.Title,
                question.Topic,
                question.Author?.DisplayName ?? string.Empty,
                question.CreatedAt,
                question.Answers.Count,
                question.Answers.Any(answer => answer.IsAccepted)))
            .ToList();

        return Task.FromResult(new QuestionPage(items, query.Page, query.PageSize, items.Count));
    }
}
