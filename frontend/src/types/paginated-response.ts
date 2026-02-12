export interface PaginatedResponse<T> {
  values: Array<T>
  totalValues: number
  currentPage: number
  totalPages: number
}