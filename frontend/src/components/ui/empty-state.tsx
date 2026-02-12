import type { ReactNode } from "react"

interface EmptyStateProps {
  icon: ReactNode
  title?: string
  description: string
}

export function EmptyState({ icon, title, description }: EmptyStateProps) {
  return (
    <div className="flex flex-col items-center gap-3 py-16 text-center">
      <div className="flex h-16 w-16 items-center justify-center rounded-2xl bg-slate-100 text-slate-400">
        {icon}
      </div>
      {title && (
        <h3 className="text-lg font-semibold text-slate-900">{title}</h3>
      )}
      <p className="max-w-sm text-sm text-slate-500">{description}</p>
    </div>
  )
}
