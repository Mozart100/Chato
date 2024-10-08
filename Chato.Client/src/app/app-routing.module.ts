import { NgModule } from '@angular/core'
import { Routes, RouterModule } from '@angular/router'
import { authGuard } from './core/guards/auth.guard'

const routes: Routes = [
    { path: '', loadChildren: () => import('./chat/chat.module').then(m => m.ChatModule), canActivate: [authGuard] },
    { path: 'account', loadChildren: () => import('./account/account.module').then(m => m.AccountModule) },
]

@NgModule({
    imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'top' })],
    exports: [RouterModule]
})
export class AppRoutingModule {
}
