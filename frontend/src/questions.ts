import type { Topic } from './topics'

export type QuestionListItem = {
  id: string
  title: string
  topic: Topic
  authorDisplayName: string
  createdAt: string
  answerCount: number
  hasAcceptedAnswer: boolean
}

export type QuestionPage = {
  items: QuestionListItem[]
  page: number
  pageSize: number
  total: number
}

export type AnswerItem = {
  id: string
  body: string
  authorDisplayName: string
  isAccepted: boolean
  createdAt: string
  updatedAt: string
  canEdit: boolean
}

export type QuestionDetails = {
  id: string
  title: string
  body: string
  topic: Topic
  authorDisplayName: string
  createdAt: string
  updatedAt: string
  answerCount: number
  hasAcceptedAnswer: boolean
  canEdit: boolean
  answers: AnswerItem[]
}
