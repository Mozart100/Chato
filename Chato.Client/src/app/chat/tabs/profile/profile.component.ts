import { Component, OnInit } from '@angular/core'
import { AuthenticationService } from '../../../core/services/auth.service'
import { NavStore } from '../../../core/store/nav.store'

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss']
})
/**
 * Tabs-Profile component
 */
export class ProfileComponent implements OnInit {

    constructor(private navStore: NavStore,
                public auth: AuthenticationService) {
        this.navStore.selectedTab.set(1)
    }

    ngOnInit(): void {
    }

}
