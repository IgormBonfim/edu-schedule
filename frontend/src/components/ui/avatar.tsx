interface AvatarProps {
  initials: string
  size?: "sm" | "md" | "lg"
  variant?: "default" | "active"
}

const sizeMap = {
  sm: "h-8 w-8 text-xs",
  md: "h-10 w-10 text-xs",
  lg: "h-11 w-11 text-sm",
}

const variantMap = {
  default: "bg-slate-100 text-slate-500",
  active: "bg-indigo-600 text-white",
}

export function Avatar({ initials, size = "md", variant = "default" }: AvatarProps) {
  return (
    <div
      className={`flex shrink-0 items-center justify-center rounded-full font-medium ${sizeMap[size]} ${variantMap[variant]}`}
    >
      {initials}
    </div>
  )
}
