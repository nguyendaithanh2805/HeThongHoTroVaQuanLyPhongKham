import { HttpInterceptorFn } from "@angular/common/http";
import { AuthService } from "../services/Auth/AuthService";
import { inject } from "@angular/core";

// tai lieu tham khao: https://v18.angular.dev/guide/http/interceptors
// https://v18.angular.dev/api/common/http/HttpInterceptorFn?tab=usage-notes
export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const authService = inject(AuthService);
    const token = authService.getToken();
  
    if (token) {
      const authReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
      return next(authReq);
    }
  
    return next(req);
  };