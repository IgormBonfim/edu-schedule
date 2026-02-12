import { useState, useRef, useEffect, type ReactNode } from "react"

interface DropdownProps {
  trigger: ReactNode
  children: ReactNode
  align?: "left" | "right"
}

export function Dropdown({ trigger, children, align = "right" }: DropdownProps) {
  const [open, setOpen] = useState(false)
  const ref = useRef<HTMLDivElement>(null)

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (ref.current && !ref.current.contains(e.target as Node)) {
        setOpen(false)
      }
    }
    document.addEventListener("mousedown", handleClickOutside)
    return () => document.removeEventListener("mousedown", handleClickOutside)
  }, [])

  return (
    <div className="relative" ref={ref}>
      <button onClick={() => setOpen(!open)} type="button">
        {trigger}
      </button>
      {open && (
        <div
          className={`absolute top-full z-50 mt-2 w-56 rounded-lg border border-slate-200 bg-white py-1 shadow-lg ${align === "right" ? "right-0" : "left-0"}`}
          onClick={() => setOpen(false)}
        >
          {children}
        </div>
      )}
    </div>
  )
}

export function DropdownItem({
  children,
  onClick,
  disabled,
  variant = "default",
}: {
  children: ReactNode
  onClick?: () => void
  disabled?: boolean
  variant?: "default" | "danger"
}) {
  const variantStyles = {
    default: disabled
      ? "text-slate-400 cursor-not-allowed"
      : "text-slate-700 hover:bg-slate-50",
    danger: "text-red-600 hover:bg-red-50",
  }

  return (
    <button
      onClick={onClick}
      disabled={disabled}
      className={`flex w-full items-center gap-2 px-3 py-2 text-sm transition-colors ${variantStyles[variant]}`}
    >
      {children}
    </button>
  )
}

export function DropdownLabel({ children }: { children: ReactNode }) {
  return (
    <div className="border-b border-slate-100 px-3 py-2.5">
      {children}
    </div>
  )
}

export function DropdownSeparator() {
  return <div className="my-1 border-t border-slate-100" />
}
