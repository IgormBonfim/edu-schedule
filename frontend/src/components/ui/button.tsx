import React from 'react';

// Tipagem das variantes para garantir consistência em todo o projeto
type ButtonVariant = 'primary' | 'secondary' | 'danger' | 'ghost';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant;
  isLoading?: boolean;
}

export const Button = ({
  children,
  variant = 'primary',
  isLoading,
  className = '',
  disabled,
  ...props
}: ButtonProps) => {
  
  const variantClasses: Record<ButtonVariant, string> = {
    primary: 'bg-primary text-white hover:bg-indigo-700 focus:ring-indigo-500',
    secondary: 'bg-secondary text-slate-900 hover:bg-slate-300 focus:ring-slate-400',
    danger: 'bg-danger text-white hover:bg-rose-700 focus:ring-rose-500',
    ghost: 'bg-transparent text-slate-600 hover:bg-slate-100 focus:ring-slate-200',
  };

  const baseClasses = 'inline-flex items-center justify-center px-4 py-2 rounded-md font-medium transition-all focus:outline-none focus:ring-2 cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed';

  return (
    <button
      {...props}
      disabled={disabled || isLoading}
      className={`${baseClasses} ${variantClasses[variant]} ${className}`}
    >
      {isLoading ? (
        <span className="flex items-center gap-2">
          <svg className="animate-spin h-4 w-4 text-current" viewBox="0 0 24 24">
            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
          </svg>
          Carregando...
        </span>
      ) : (
        children
      )}
    </button>
  );
};