import { useEffect, type ReactNode } from "react"
import { X } from "lucide-react"

interface SlideOverProps {
  open: boolean
  onClose: () => void
  title: string
  children: ReactNode
}

export function SlideOver({ open, onClose, title, children }: SlideOverProps) {
  useEffect(() => {
    if (open) {
      document.body.style.overflow = "hidden"
    } else {
      document.body.style.overflow = ""
    }
    return () => {
      document.body.style.overflow = ""
    }
  }, [open])

  if (!open) return null

  return (
    <>
      <div
        className="fixed inset-0 z-40 bg-black/40 md:hidden"
        onClick={onClose}
        aria-hidden="true"
      />
      <div className="fixed inset-y-0 left-0 z-50 flex w-[340px] flex-col bg-white shadow-xl md:hidden animate-in slide-in-from-left duration-200">
        <div className="flex items-center justify-between border-b border-slate-200 px-4 py-3">
          <h2 className="text-sm font-semibold text-slate-900">{title}</h2>
          <button
            onClick={onClose}
            className="rounded-md p-1 text-slate-400 hover:bg-slate-100 hover:text-slate-900"
            aria-label="Fechar painel"
          >
            <X className="h-5 w-5" />
          </button>
        </div>
        <div className="flex-1 overflow-hidden">{children}</div>
      </div>
    </>
  )
}
