import { useState, useEffect, useCallback, useRef, forwardRef } from "react"
import { Search, Users } from "lucide-react"
import type { Student } from "../types/student"
import { Input } from "./ui/input"
import { Avatar } from "./ui/avatar"
import { Skeleton } from "./ui/skeleton"
import { EmptyState } from "./ui/empty-state"
import { api } from "../api/axios-client"
import { Badge } from "./ui/badge"
import type { PaginatedResponse } from "../types/paginated-response"

interface StudentsListProps {
  selectedStudent: Student | null
  onSelectStudent: (student: Student) => void
}

function getInitials(name: string) {
  return name.split(" ").map((n) => n[0]).slice(0, 2).join("").toUpperCase()
}

export function StudentsList({ selectedStudent, onSelectStudent }: StudentsListProps) {
  const pageRef = useRef(1);
  const [studentsResponse, setStudentsResponse] = useState<PaginatedResponse<Student>>()
  const [students, setStudents] = useState<Student[]>([]);
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true)
  const [search, setSearch] = useState("")
  const observer = useRef<IntersectionObserver | null>(null);

  const getStudents = useCallback(async (isNextPage = false) => {
    setIsLoading(true)
    try {
        const pageToFetch = isNextPage ? pageRef.current : 1;      
        const params: any = {
        itemsPerPage: 50,
        page: pageToFetch,
      };

      if (search) {
        // 1. Verifica se é um ID numérico (Apenas dígitos)
        if (/^\d+$/.test(search)) {
          params.id = parseInt(search);
        } 
        // 2. Verifica se é um GUID/UUID (Formato do ExternalId)
        else if (/^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/.test(search)) {
          params.externalId = search;
        } 
        // 3. Verifica se tem formato de e-mail
        else if (search.includes('@')) {
          params.email = search;
        }
        // 4. Caso contrário, trata como busca por nome
        else {
          params.displayName = search;
        }
      }

      const { data } = await api.get("/students", { params });
      setStudentsResponse(data)
      setStudents(prev => isNextPage ? [...prev, ...data.values] : data.values);
    } catch (err) {
      console.error("Failed to fetch users:", err)
    } finally {
      setIsLoading(false)
    }
  }, [search])

const lastStudentElementRef = useCallback((node: HTMLDivElement) => {
  if (isLoading) return;
  if (observer.current) observer.current.disconnect();

  observer.current = new IntersectionObserver(entries => {
    if (entries[0].isIntersecting) {
      setPage(currentPage => {        
        if (studentsResponse && currentPage < studentsResponse.totalPages) {
          return currentPage + 1;
        }
        return currentPage;
      });
    }
  }, { threshold: 0.1 });

  if (node) observer.current.observe(node);
}, [isLoading, studentsResponse])

  useEffect(() => {
    setIsLoading(true);
    pageRef.current = 1;
    setPage(1);
    setStudents([]);
    const timer = setTimeout(getStudents, 300)
    return () => clearTimeout(timer)
  }, [search])

useEffect(() => {
    if (page > 1) {
      pageRef.current = page;
      getStudents(true);
    }
}, [page]);

  return (
    <div className="flex h-full flex-col">
      <div className="border-b border-slate-200/60 p-4">
        <div className="mb-3 flex items-center gap-2">
          <Users className="h-5 w-5 text-indigo-600" />
          <h2 className="text-base font-semibold text-slate-900">Estudantes</h2>
          {!isLoading && (
            <Badge className="ml-auto px-2 py-0.5 text-xs">
              { studentsResponse?.totalValues }
            </Badge>
          )}
        </div>

        <div className="flex flex-col gap-2">
            <div className="relative">
                <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400" />
                <Input
                    placeholder="Buscar por nome, email..."
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                    className="pl-9"
                    autoComplete="false"
                />
            </div>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto">
        <div className="flex flex-col gap-1 p-2">
          {isLoading && students.length === 0 && <ListSkeleton />}

          {!isLoading && students.length === 0 && (
            <EmptyState icon={<Users className="h-8 w-8" />} description="Nenhum estudante encontrado" />
          )}

          {students.map((student, index) => (
            <StudentListItem
              key={student.id}
              student={student}
              isSelected={selectedStudent?.id === student.id}
              onSelect={onSelectStudent}
              ref={index === students.length - 1 ? lastStudentElementRef : null}
            />
          ))}

          {isLoading && page > 1 && (
            <div className="flex items-center gap-3 rounded-lg p-3">
              <Skeleton className="h-10 w-10 rounded-full" />
              <div className="flex flex-1 flex-col gap-1.5">
                <Skeleton className="h-4 w-32" />
                <Skeleton className="h-3 w-48" />
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

const ListSkeleton = ({ count = 10 }: { count?: number }) => (
  <>
    {Array.from({ length: count }).map((_, i) => (
      <div key={i} className="flex items-center gap-3 rounded-lg p-3">
        <Skeleton className="h-10 w-10 rounded-full" />
        <div className="flex flex-1 flex-col gap-1.5">
          <Skeleton className="h-4 w-32" />
          <Skeleton className="h-3 w-48" />
        </div>
      </div>
    ))}
  </>
);

const StudentListItem = forwardRef<HTMLDivElement, { 
  student: Student; 
  isSelected: boolean; 
  onSelect: (s: Student) => void 
}>(({ student, isSelected, onSelect }, ref) => (
  <div ref={ref} className="cursor-pointer">
    <button
      onClick={() => onSelect(student)}
      className={`group flex w-full items-start gap-3 rounded-lg p-3 text-left transition-colors ${
        isSelected ? "bg-indigo-50 ring-1 ring-indigo-600/20" : "hover:bg-slate-50"
      }`}
    >
      <Avatar initials={getInitials(student.displayName)} variant={isSelected ? "active" : "default"} />
      <div className="flex min-w-0 flex-1 flex-col gap-0.5">
        <div className="flex items-center gap-2">
          <span className="truncate text-sm font-medium text-slate-900">{student.displayName}</span>
        </div>
        <span className="truncate text-xs text-slate-500">{student.email}</span>
      </div>
    </button>
  </div>
));