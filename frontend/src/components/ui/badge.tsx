import type { ReactNode } from "react"

interface BadgeProps {
  children: ReactNode
  variant?: "default" | "primary" | "outline" | "count"
  className?: string
}

const variantMap = {
  default: "bg-slate-100 text-slate-600",
  primary: "bg-indigo-600 text-white",
  outline: "border border-slate-300 text-slate-700 bg-white",
  count: "bg-slate-100 text-slate-600",
}

export function Badge({ children, variant = "default", className = "" }: BadgeProps) {
  return (
    <span
      className={`inline-flex shrink-0 items-center rounded-md px-1.5 py-0.5 text-[10px] font-medium leading-4 ${variantMap[variant]} ${className}`}
    >
      {children}
    </span>
  )
}
