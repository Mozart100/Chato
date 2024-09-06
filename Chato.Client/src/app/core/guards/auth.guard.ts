import { inject } from '@angular/core'
import { Router, CanActivateFn } from '@angular/router'
import { AuthenticationService } from '../services/auth.service'
import { SAVED_TOKEN_KEY } from '../helpers/consts'

export const authGuard: CanActivateFn = (route, state) => {
    const router = inject(Router)

    // check if we have token
    const token = sessionStorage.getItem(SAVED_TOKEN_KEY)
    if (!token || token === 'undefined') {
        router.navigate(['/account/login'], { queryParams: { returnUrl: state.url } })
        return false
    }

    // if we have token, check its validity
    const authService = inject(AuthenticationService)
    return authService.loadUser()
        .then(isAuth => {
            if (!isAuth) {
                router.navigate(['/account/login'], { queryParams: { returnUrl: state.url } })
            }
            return isAuth
        })
}
