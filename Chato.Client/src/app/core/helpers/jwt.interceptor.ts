import { Injectable } from '@angular/core'
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http'
import { Observable } from 'rxjs'
import { SAVED_TOKEN_KEY } from './consts'

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const token = sessionStorage.getItem(SAVED_TOKEN_KEY)
        request = request.clone({
            setHeaders: {
                Authorization: `Bearer ${token}`
            }
        })
        return next.handle(request)
    }
}
