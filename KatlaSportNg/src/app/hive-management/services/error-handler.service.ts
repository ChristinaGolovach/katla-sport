import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {

  constructor() { }

  handleError(error: any): string {
    
    var httpError = new HttpErrorResponse(error);

    if (httpError.error instanceof ProgressEvent){
      return "Sorry, the network error occured. Try later, please.";             
    } 
    else{
      switch(httpError.status){
        case 400: 
          return "Sorry, your request is bad. Please, enter correct data and try again.";
        case 404:
          return "Sorry, the resource not found.";
        case 409:
          return "Sorry, the conflict error occured. Please, chek your data and try again.";
        case 500:
          return "Sorry, the server error occured. Try later, please.";
        default:
          return "Unknown error. Try later, please.";        
      }
    }
  }
}
