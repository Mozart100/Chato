import { Component } from '@angular/core'
import { NavStore } from '../../core/store/nav.store'

@Component({
    selector: 'app-chats',
    standalone: true,
    imports: [],
    templateUrl: './chats.component.html',
    styleUrl: './chats.component.scss'
})
export class ChatsComponent {

    constructor(private navStore: NavStore) {
        this.navStore.selectedTab.set(2)
    }
}
