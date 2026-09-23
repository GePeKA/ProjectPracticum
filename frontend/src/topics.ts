export const topicValues = ['Study', 'Everyday', 'City', 'Tech', 'Other'] as const

export type Topic = (typeof topicValues)[number]
