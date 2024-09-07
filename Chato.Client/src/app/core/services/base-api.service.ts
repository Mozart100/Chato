import { HttpClient } from '@angular/common/http'
import { firstValueFrom, throwError } from 'rxjs'
import { ToastrService } from 'ngx-toastr'

export abstract class BaseApiService {

    protected constructor(protected http: HttpClient,
                          protected alert: ToastrService) {
    }

    protected sendGet<T>(url: string): Promise<T> {
        return new Promise((resolve, reject) => {
            const req = this.http.get<T>(url)
            firstValueFrom(req)
                .then(res => resolve(res))
                .catch(e => {
                    this.alert.error(e.error?.body?.Reason || e.error.title || e.error.error, 'Error', { disableTimeOut: true })
                    reject(e)
                })
        })
    }

    protected sendPost<T, Y>(url: string, data: T): Promise<Y> {
        return new Promise((resolve, reject) => {
            const req = this.http.post(url, data)
            firstValueFrom(req)
                .then((res: Y) => resolve(res))
                .catch(e => {
                    this.alert.error(e.error?.body?.Reason || e.error.title || e.error.error, 'Error', { disableTimeOut: true })
                    reject(e)
                })
        })
    }

}
