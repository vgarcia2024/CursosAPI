import axios from "axios";

// El back de /UTN (CursosAPI) autentica por COOKIE (no JWT), por eso
// no hay token que guardar: alcanza con withCredentials para que el navegador
// mande la cookie de sesión en cada request, y que el back la pueda leer.
export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "https://localhost:7000",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
    "ngrok-skip-browser-warning": "true",
  },
});

// Normaliza los errores del back (ErrorResponse / ResponseValidation / ResponseMessage)
// a una forma única y predecible para usar en toda la app.
export function parseApiError(err) {
  if (axios.isAxiosError(err)) {
    const status = err.response?.status ?? 0;
    const data = err.response?.data;

    if (status === 400 && data && typeof data === "object" && "errors" in data) {
      // ResponseValidation: { errors: { campo: ["msg1", "msg2"] } }
      const errors = data.errors;
      const first = Object.values(errors)[0]?.[0];
      return {
        status,
        message: first || "Los datos ingresados no son válidos.",
        validation: errors,
      };
    }

    if (typeof data === "string") {
      return { status, message: data };
    }
    if (data && typeof data === "object" && "message" in data) {
      return { status, message: data.message };
    }

    if (status === 401) return { status, message: "Tu sesión expiró. Iniciá sesión de nuevo." };
    if (status === 403) return { status, message: "No tenés permiso para hacer esto." };
    if (status === 0) return { status, message: "No se pudo conectar con el servidor." };

    return { status, message: "Ocurrió un error inesperado. Probá de nuevo." };
  }
  return { status: 0, message: "Ocurrió un error inesperado. Probá de nuevo." };
}
