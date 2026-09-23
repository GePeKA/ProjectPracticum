using Sprosi.Application.Abstractions;
using Sprosi.Domain;

namespace Sprosi.Application.Tests.Fakes;

internal sealed class FakeAnswerRepository : IAnswerRepository
{
    private readonly FakeQuestionRepository _questions;

    public FakeAnswerRepository(FakeQuestionRepository questions)
    {
        _questions = questions;
    }

    public Task<Answer?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_questions.Questions.SelectMany(question => question.Answers).FirstOrDefault(answer => answer.Id == id));

    public Task AddAsync(Answer answer, CancellationToken cancellationToken)
    {
        var question = _questions.Questions.First(item => item.Id == answer.QuestionId);
        answer.Author ??= new User { Id = answer.AuthorId, DisplayName = "Автор" };
        answer.Question = question;
        question.Answers.Add(answer);
        return Task.CompletedTask;
    }

    public Task SaveAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task RemoveAsync(Answer answer, CancellationToken cancellationToken)
    {
        answer.Question?.Answers.Remove(answer);
        foreach (var question in _questions.Questions)
            question.Answers.Remove(answer);

        return Task.CompletedTask;
    }
}
