import { GraduationCap, ChevronDown, LogOut } from "lucide-react"
import { useAuth } from "../context/auth-context"
import { Dropdown, DropdownItem, DropdownLabel } from "./ui/dropdown"
import { Avatar } from "./ui/avatar"

function getInitials(name: string) {
  return name
    .split(" ")
    .map((n) => n[0])
    .slice(0, 2)
    .join("")
    .toUpperCase()
}

export function Header() {
  const { user, logout } = useAuth()
  const initials = user?.name ? getInitials(user.name) : "U"

  return (
    <header className="sticky top-0 z-30 flex h-16 items-center justify-between border-b border-slate-200/60 bg-white px-4 md:px-6">
      <div className="flex items-center gap-3">
        <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-indigo-600 text-white">
          <GraduationCap className="h-5 w-5" />
        </div>
        <div>
          <h1 className="text-base font-semibold leading-tight text-slate-900">
            EduSchedule
          </h1>
          <p className="hidden text-xs text-slate-500 sm:block">
            Gerenciamento de Agendas
          </p>
        </div>
      </div>

      <div className="flex items-center gap-3">
        <div className="hidden text-right sm:block">
          <p className="text-sm font-medium text-slate-900">{user?.name}</p>
          <p className="text-xs text-slate-500">{user?.role}</p>
        </div>

        <Dropdown
          trigger={
            <span className="flex items-center gap-1.5 rounded-full p-0.5 transition-colors hover:bg-slate-100">
              <Avatar initials={initials} size="sm" variant="active" />
              <ChevronDown className="h-4 w-4 text-slate-400" />
            </span>
          }
        >
          <DropdownLabel>
            <p className="text-sm font-medium text-slate-900">{user?.name}</p>
            <p className="text-xs text-slate-500">{user?.email}</p>
          </DropdownLabel>
          <DropdownItem variant="danger" onClick={logout}>
            <LogOut className="h-4 w-4" />
            Sair
          </DropdownItem>
        </Dropdown>
      </div>
    </header>
  )
}
