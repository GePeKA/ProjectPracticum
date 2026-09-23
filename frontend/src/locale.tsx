import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from 'react'

export type Language = 'ru' | 'en'

export const timeZoneChoices = [
  'UTC',
  'Europe/Kaliningrad',
  'Europe/Moscow',
  'Asia/Yekaterinburg',
  'Asia/Novosibirsk',
  'Asia/Vladivostok',
  'Europe/London',
  'Europe/Berlin',
  'America/New_York',
  'Asia/Tokyo',
]

const dictionaries = {
  ru: {
    feed: 'Лента',
    questions: 'Вопросы',
    ask: 'Задать вопрос',
    search: 'Поиск по заголовку',
    allTopics: 'Все темы',
    all: 'Все',
    waiting: 'Ждут ответа',
    resolved: 'Есть лучший ответ',
    sortNew: 'Сначала новые',
    sortOld: 'Сначала старые',
    sortPopular: 'Больше ответов',
    empty: 'Пока ничего не нашлось.',
    back: 'Назад',
    next: 'Дальше',
    of: 'из',
    signIn: 'Войти',
    signOut: 'Выйти',
    register: 'Регистрация',
    name: 'Имя',
    password: 'Пароль',
    createAccount: 'Создать аккаунт',
    noAccount: 'Нет аккаунта',
    haveAccount: 'Уже есть аккаунт',
    signInFailed: 'Не получилось войти.',
    registerFailed: 'Не получилось зарегистрироваться.',
    newQuestion: 'Новый вопрос',
    editQuestion: 'Правка вопроса',
    title: 'Заголовок',
    topic: 'Тема',
    text: 'Текст',
    save: 'Сохранить',
    openFailed: 'Не получилось открыть вопрос.',
    saveFailed: 'Не получилось сохранить вопрос.',
    loadFailed: 'Не получилось загрузить вопросы.',
    answers: 'Ответы',
    noAnswers: 'Пока никто не ответил.',
    bestAnswer: 'Лучший ответ',
    edit: 'Изменить',
    delete: 'Удалить',
    cancel: 'Отмена',
    yourAnswer: 'Ваш ответ',
    reply: 'Ответить',
    signInToReply: 'Войдите',
    signInToReplyTail: ', чтобы ответить.',
    markBest: 'Отметить лучшим',
    clearMark: 'Снять отметку',
    loading: 'Загрузка…',
    deleteFailed: 'Не получилось удалить вопрос.',
    changeFailed: 'Не получилось сохранить изменение.',
    settings: 'Настройки',
    settingsLead: 'Язык меняет подписи. Часовой пояс меняет только показ времени: в базе момент хранится в UTC.',
    language: 'Язык',
    timeZone: 'Часовой пояс',
    timeSample: 'Сейчас',
    waitingShort: 'Ждёт ответа',
    resolvedShort: 'Есть лучший ответ',
    'topic.Study': 'Учёба',
    'topic.Everyday': 'Быт',
    'topic.City': 'Город',
    'topic.Tech': 'Техника',
    'topic.Other': 'Другое',
  },
  en: {
    feed: 'Board',
    questions: 'Questions',
    ask: 'Ask a question',
    search: 'Search by title',
    allTopics: 'All topics',
    all: 'All',
    waiting: 'Waiting for an answer',
    resolved: 'Has a best answer',
    sortNew: 'Newest first',
    sortOld: 'Oldest first',
    sortPopular: 'Most answers',
    empty: 'Nothing matched.',
    back: 'Back',
    next: 'Next',
    of: 'of',
    signIn: 'Sign in',
    signOut: 'Sign out',
    register: 'Register',
    name: 'Name',
    password: 'Password',
    createAccount: 'Create account',
    noAccount: 'No account yet',
    haveAccount: 'Already have an account',
    signInFailed: 'Could not sign in.',
    registerFailed: 'Could not register.',
    newQuestion: 'New question',
    editQuestion: 'Edit question',
    title: 'Title',
    topic: 'Topic',
    text: 'Text',
    save: 'Save',
    openFailed: 'Could not open the question.',
    saveFailed: 'Could not save the question.',
    loadFailed: 'Could not load questions.',
    answers: 'Answers',
    noAnswers: 'No answers yet.',
    bestAnswer: 'Best answer',
    edit: 'Edit',
    delete: 'Delete',
    cancel: 'Cancel',
    yourAnswer: 'Your answer',
    reply: 'Reply',
    signInToReply: 'Sign in',
    signInToReplyTail: ' to reply.',
    markBest: 'Mark as best',
    clearMark: 'Clear mark',
    loading: 'Loading…',
    deleteFailed: 'Could not delete the question.',
    changeFailed: 'Could not save the change.',
    settings: 'Settings',
    settingsLead: 'Language changes the labels. The time zone only changes how a moment is shown: the database keeps it in UTC.',
    language: 'Language',
    timeZone: 'Time zone',
    timeSample: 'Now',
    waitingShort: 'Waiting',
    resolvedShort: 'Best answer chosen',
    'topic.Study': 'Study',
    'topic.Everyday': 'Everyday',
    'topic.City': 'City',
    'topic.Tech': 'Tech',
    'topic.Other': 'Other',
  },
} as const

export type MessageKey = keyof typeof dictionaries.ru

const storageKey = 'sprosi-locale'

type LocaleState = {
  language: Language
  timeZone: string
}

type LocaleValue = LocaleState & {
  setLanguage: (language: Language) => void
  setTimeZone: (timeZone: string) => void
  t: (key: MessageKey) => string
  formatWhen: (value: string) => string
  topicLabel: (topic: string) => string
  answersLabel: (count: number) => string
}

const LocaleContext = createContext<LocaleValue | null>(null)

function browserTimeZone() {
  return Intl.DateTimeFormat().resolvedOptions().timeZone || 'UTC'
}

function readLocale(): LocaleState {
  const fallback = { language: 'ru' as Language, timeZone: browserTimeZone() }
  const raw = localStorage.getItem(storageKey)
  if (!raw)
    return fallback

  try {
    const parsed = JSON.parse(raw) as Partial<LocaleState>
    return {
      language: parsed.language === 'en' ? 'en' : 'ru',
      timeZone: parsed.timeZone || fallback.timeZone,
    }
  }
  catch {
    return fallback
  }
}

export function LocaleProvider({ children }: { children: ReactNode }) {
  const [locale, setLocale] = useState<LocaleState>(readLocale)

  useEffect(() => {
    localStorage.setItem(storageKey, JSON.stringify(locale))
    document.documentElement.lang = locale.language
  }, [locale])

  const value = useMemo<LocaleValue>(() => {
    const dictionary = dictionaries[locale.language]
    return {
      ...locale,
      setLanguage(language) {
        setLocale(current => ({ ...current, language }))
      },
      setTimeZone(timeZone) {
        setLocale(current => ({ ...current, timeZone }))
      },
      t(key) {
        return dictionary[key]
      },
      formatWhen(value) {
        return new Intl.DateTimeFormat(locale.language === 'en' ? 'en-GB' : 'ru-RU', {
          day: 'numeric',
          month: 'long',
          hour: '2-digit',
          minute: '2-digit',
          timeZone: locale.timeZone,
          timeZoneName: 'short',
        }).format(new Date(value))
      },
      topicLabel(topic) {
        const key = `topic.${topic}` as MessageKey
        return key in dictionary ? dictionary[key] : topic
      },
      answersLabel(count) {
        if (locale.language === 'en')
          return count === 1 ? 'answer' : 'answers'

        const mod10 = count % 10
        const mod100 = count % 100
        if (mod10 === 1 && mod100 !== 11)
          return 'ответ'
        if (mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14))
          return 'ответа'
        return 'ответов'
      },
    }
  }, [locale])

  return <LocaleContext.Provider value={value}>{children}</LocaleContext.Provider>
}

export function useLocale() {
  const value = useContext(LocaleContext)
  if (!value)
    throw new Error('LocaleProvider is missing')

  return value
}
