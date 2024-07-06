import { NgModule } from '@angular/core'
import { Routes, RouterModule } from '@angular/router'

import { LoginComponent } from './login/login.component'
import { LockscreenComponent } from './lockscreen/lockscreen.component'

const routes: Routes = [
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'lock-screen',
        component: LockscreenComponent
    }
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AuthRoutingModule {
}
