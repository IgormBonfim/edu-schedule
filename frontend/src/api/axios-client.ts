// src/api/axios-client.ts
import axios from 'axios';
import type { AuthState } from '../types/auth-state';

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5077/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use((config) => {
  const stored = sessionStorage.getItem("eduschedule_auth");
  
  if (stored) {
    try {
      const authData = JSON.parse(stored) as AuthState;
      if (authData.token && config.headers) {
        config.headers.Authorization = `Bearer ${authData.token}`;
      }
    } catch (error) {
      console.error("Erro ao ler token para requisição", error);
    }
  }

  return config;
}, (error) => {
  return Promise.reject(error);
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      sessionStorage.removeItem("eduschedule_auth");
      if (typeof window !== 'undefined') {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);