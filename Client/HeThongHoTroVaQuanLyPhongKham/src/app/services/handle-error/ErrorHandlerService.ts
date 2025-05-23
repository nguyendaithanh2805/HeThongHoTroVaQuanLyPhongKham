import { HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { IErrorHandler } from "./IErrorHandler";

@Injectable(
    { providedIn: 'root' }
)
export class ErrorHandlerService implements IErrorHandler {
    handleError(error: HttpErrorResponse): string[] {
        const errorMessages: string[] = [];
    
        if (error.status === 400 && error.error) {
          if (typeof error.error === 'string') {
            errorMessages.push(error.error);
          } else if (error.error.errors) {
            const errorObj = error.error.errors;
            Object.keys(errorObj).forEach(key => {
              errorMessages.push(...errorObj[key]);
            });
          } else if (error.error.message) {
            errorMessages.push(error.error.message);
          }
        } else if (error.error?.message) {
          errorMessages.push(error.error.message);
        } else {
          errorMessages.push('Đã xảy ra lỗi không xác định. Vui lòng thử lại sau.');
        }
    
        return errorMessages;
      }
}