import { useState } from "react"
import { Header } from "../components/header"
import { Users } from "lucide-react"
import type { Student } from "../types/student"
import { SlideOver } from "../components/ui/slide-over"
import { StudentsList } from "../components/students-list"
import { EventViewer } from "../components/event-viewer"

export function DashboardPage() {
    const [selectedStudent, setSelectedStudent] = useState<Student | null>(null)
    const [mobileOpen, setMobileOpen] = useState(false)

    const handleSelectStudent = (user: Student) => {
        setSelectedStudent(user)
        setMobileOpen(false)
    }

    return (
        <div className="flex h-screen flex-col">
            <Header />
            <div className="flex flex-1 overflow-hidden">
                {/* Desktop */}
                <aside className="hidden w-95 shrink-0 border-r border-slate-200/60 bg-white md:flex md:flex-col lg:w-105">
                    <StudentsList selectedStudent={selectedStudent} onSelectStudent={handleSelectStudent} />
                </aside>

                {/* Mobile button */}
                <button
                    onClick={() => setMobileOpen(true)}
                    className="fixed bottom-4 left-4 z-40 flex items-center gap-2 rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm font-medium text-slate-900 shadow-lg transition-colors hover:bg-slate-50 md:hidden"
                >
                    <Users className="h-4 w-4" />
                    Usuarios
                </button>

                {/* Mobile list*/}
                <SlideOver open={mobileOpen} onClose={() => setMobileOpen(false)} title="Estudantes">
                    <StudentsList selectedStudent={selectedStudent} onSelectStudent={handleSelectStudent} />
                </SlideOver>

                <main className="flex flex-1 flex-col overflow-hidden bg-slate-50">
                    <EventViewer student={selectedStudent} />
                </main>
            </div>
        </div>
    )
}
