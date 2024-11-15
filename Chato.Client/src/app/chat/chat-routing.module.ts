import { NgModule } from '@angular/core'
import { Routes, RouterModule } from '@angular/router'

import { IndexComponent } from './index/index.component'
import { MainComponent } from './main/main.component'
import { RoomsComponent } from './rooms/rooms.component'
import { ChatsComponent } from './chats/chats.component'
import { ProfileComponent } from './tabs/profile/profile.component'

const routes: Routes = [
    { path: 'old', component: IndexComponent },
    {
        path: '', component: MainComponent, children: [
            { path: '', redirectTo: 'rooms', pathMatch: 'full' },
            { path: 'rooms', component: RoomsComponent },
            { path: 'chats', component: ChatsComponent },
            { path: 'profile', component: ProfileComponent },
        ]
    }
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class ChatRoutingModule {
}
