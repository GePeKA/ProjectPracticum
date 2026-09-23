export const topicOptions = [
  { value: 'Study', label: 'Учёба' },
  { value: 'Everyday', label: 'Быт' },
  { value: 'City', label: 'Город' },
  { value: 'Tech', label: 'Техника' },
  { value: 'Other', label: 'Другое' },
] as const

export type Topic = (typeof topicOptions)[number]['value']

export function topicLabel(topic: string) {
  return topicOptions.find(item => item.value === topic)?.label ?? topic
}

export function formatWhen(value: string) {
  return new Intl.DateTimeFormat('ru-RU', {
    day: 'numeric',
    month: 'long',
    hour: '2-digit',
    minute: '2-digit',
  }).format(new Date(value))
}
