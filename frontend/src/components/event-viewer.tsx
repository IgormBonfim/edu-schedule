import { useState, useEffect, useCallback } from "react"
import { CalendarDays, Clock, Calendar } from "lucide-react"
import type { Student } from "../types/student"
import type { Evento } from "../types/event"
import { EmptyState } from "./ui/empty-state"
import { Avatar } from "./ui/avatar"
import { Skeleton } from "./ui/skeleton"
import { api } from "../api/axios-client"

interface EventViewerProps {
  student: Student | null
}

function formatTime(dateStr: string): string {
  const d = new Date(dateStr)
  return d.toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" })
}

function isSameDay(a: Date, b: Date) {
  return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate()
}

function getDayLabel(dateStr: string): string {
  const date = new Date(dateStr)
  const now = new Date()
  const tomorrow = new Date(now)
  tomorrow.setDate(now.getDate() + 1)

  if (isSameDay(date, now)) return "Hoje"
  if (isSameDay(date, tomorrow)) return "Amanhã"

  return date.toLocaleDateString("pt-BR", {
    weekday: "long",
    day: "2-digit",
    month: "long",
  })
}

function toDateKey(dateStr: string): string {
  const d = new Date(dateStr)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`
}

function groupEventsByDay(events: Evento[]): Record<string, Evento[]> {
  const groups: Record<string, Evento[]> = {}
  for (const event of events) {
    const key = toDateKey(event.startTime)
    if (!groups[key]) groups[key] = []
    groups[key].push(event)
  }
  return groups
}

function getHoursDiff(startStr: string, endStr: string): number {
  return Math.round((new Date(endStr).getTime() - new Date(startStr).getTime()) / 3600000)
}

function isPast(dateStr: string): boolean {
  return new Date(dateStr) < new Date()
}

function getInitials(name: string) {
  return name.split(" ").map((n) => n[0]).slice(0, 2).join("").toUpperCase()
}

export function EventViewer({ student }: EventViewerProps) {
  const [events, setEvents] = useState<Evento[]>([])
  const [isLoading, setIsLoading] = useState(false)
  const [selectedEvent, setSelectedEvent] = useState<Evento | null>(null)

  const getStudentEvents = useCallback(async () => {
    if (!student?.id) return;
    
    setIsLoading(true);
    setSelectedEvent(null);
    
    try {
      const response = await api.get(`/students/${student.id}/events`);
      
      setEvents(response.data ?? []);
      
    } catch (err) {
      console.error("Failed to fetch events:", err);
      setEvents([]);
    } finally {
      setIsLoading(false);
    }
  }, [student]);

  useEffect(() => {
    getStudentEvents()
  }, [getStudentEvents])

  if (!student) {
    return (
      <div className="flex h-full items-center justify-center p-8">
        <EmptyState
          icon={<CalendarDays className="h-10 w-10" />}
          title="Selecione um estudante"
          description="Escolha um estudante na lista ao lado para visualizar todos os seus eventos programados."
        />
      </div>
    )
  }

  const grouped = groupEventsByDay(events)
  const sortedDays = Object.keys(grouped).sort()

  return (
    <div className="flex h-full flex-col">
      <div className="border-b border-slate-200/60 bg-white p-4">
        <div className="flex items-center gap-3">
          <Avatar initials={getInitials(student.displayName)} size="lg" variant="active" />
          <div className="min-w-0 flex-1">
            <h2 className="truncate text-base font-semibold text-slate-900">
              {student.displayName}
            </h2>
          </div>
          <span className="flex shrink-0 items-center gap-1.5 rounded-lg bg-slate-100 px-2.5 py-1 text-xs font-medium text-slate-600">
            <Calendar className="h-3.5 w-3.5" />
            {events.length} evento{events.length !== 1 ? "s" : ""}
          </span>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto">
        <div className="p-4">
          {isLoading ? (
            <div className="flex flex-col gap-4">
              {Array.from({ length: 4 }).map((_, i) => (
                <div key={i} className="flex flex-col gap-2">
                  <Skeleton className="h-4 w-32" />
                  <Skeleton className="h-20 w-full rounded-lg" />
                </div>
              ))}
            </div>
          ) : events.length === 0 ? (
            <EmptyState
              icon={<CalendarDays className="h-8 w-8" />}
              description="Nenhum evento encontrado para este usuario"
            />
          ) : (
            <div className="flex flex-col gap-6">
              {sortedDays.map((day) => (
                <section key={day}>
                  <div className="mb-3 flex items-center gap-2">
                    <h3 className="text-sm font-semibold capitalize text-slate-900">
                      {getDayLabel(grouped[day][0].startTime)}
                    </h3>
                    <div className="h-px flex-1 bg-slate-200" />
                    <span className="text-xs text-slate-400">
                      {grouped[day].length} evento{grouped[day].length !== 1 ? "s" : ""}
                    </span>
                  </div>

                  <div className="flex flex-col gap-2">
                    {grouped[day].map((event) => {
                      const eventIsPast = isPast(event.endTime)
                      const isSelected = selectedEvent?.id === event.id

                      return (
                        <div
                          key={event.id}
                          role="button"
                          tabIndex={0}
                          onKeyDown={(e) => {
                            if (e.key === "Enter" || e.key === " ") {
                              e.preventDefault()
                              setSelectedEvent(isSelected ? null : event)
                            }
                          }}
                          onClick={() => setSelectedEvent(isSelected ? null : event)}
                          className={`cursor-pointer rounded-xl border bg-white transition-all ${
                            eventIsPast ? "opacity-60" : ""
                          } ${
                            isSelected
                              ? "ring-2 ring-indigo-600/30 border-indigo-200 shadow-md"
                              : "border-slate-200 hover:shadow-sm hover:border-slate-300"
                          }`}
                        >
                          <div className="p-3">
                            <div className="flex items-start gap-3">
                              <div className="flex w-14 shrink-0 flex-col items-center gap-0.5 pt-0.5">
                                <span className="text-sm font-semibold text-slate-900">
                                  {formatTime(event.startTime)}
                                </span>
                                <div className="h-3 w-px bg-slate-200" />
                                <span className="text-xs text-slate-400">
                                  {formatTime(event.endTime)}
                                </span>
                              </div>

                              <div className="min-w-0 flex-1">
                                <h4 className="text-sm font-medium text-slate-900">
                                  {event.subject}
                                </h4>

                                <div className="mt-1.5 flex flex-wrap items-center gap-x-3 gap-y-1 text-xs text-slate-500">
                                  <span className="flex items-center gap-1">
                                    <Clock className="h-3 w-3 shrink-0" />
                                    {getHoursDiff(event.startTime, event.endTime)}h
                                  </span>
                                </div>
                              </div>
                            </div>
                          </div>
                        </div>
                      )
                    })}
                  </div>
                </section>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
