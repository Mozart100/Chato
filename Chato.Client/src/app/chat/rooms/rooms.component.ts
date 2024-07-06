import { Component } from '@angular/core'
import { NavStore } from '../../core/store/nav.store'

@Component({
    selector: 'app-rooms',
    standalone: true,
    imports: [],
    templateUrl: './rooms.component.html',
    styleUrl: './rooms.component.scss'
})
export class RoomsComponent {

    constructor(private navStore: NavStore) {
        this.navStore.selectedTab.set(3)
    }
}
