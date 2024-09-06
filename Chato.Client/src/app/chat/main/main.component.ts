import { Component } from '@angular/core'
import { NgbDropdown, NgbDropdownMenu, NgbDropdownToggle, NgbTooltip } from '@ng-bootstrap/ng-bootstrap'
import { TranslateModule } from '@ngx-translate/core'
import { AuthenticationService } from '../../core/services/auth.service'
import { LangService } from '../../core/services/lang.service'
import { NgClass } from '@angular/common'
import { CarouselModule } from 'ngx-owl-carousel-o'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { SimplebarAngularModule } from 'simplebar-angular'
import { TabsModule } from '../tabs/tabs.module'
import { Router, RouterOutlet } from '@angular/router'
import { NavStore } from '../../core/store/nav.store'
import { ChatWindowComponent } from '../chat-window/chat-window.component'

@Component({
    selector: 'app-main',
    standalone: true,
    imports: [
        NgbDropdown,
        NgbDropdownMenu,
        NgbDropdownToggle,
        NgbTooltip,
        TranslateModule,
        NgClass,
        CarouselModule,
        FormsModule,
        ReactiveFormsModule,
        SimplebarAngularModule,
        TabsModule,
        RouterOutlet,
        ChatWindowComponent
    ],
    templateUrl: './main.component.html',
    styleUrl: './main.component.scss'
})
export class MainComponent {

    constructor(private auth: AuthenticationService,
                public langService: LangService,
                public navStore: NavStore,
                private router: Router) {
    }

    logout() {
        this.auth.logout()
    }

    /**
     * Topbar Light-Dark Mode Change
     */
    changeMode(mode: string) {
        switch (mode) {
            case 'light':
                document.body.setAttribute('data-bs-theme', 'light')
                break
            case 'dark':
                document.body.setAttribute('data-bs-theme', 'dark')
                break
            default:
                document.body.setAttribute('data-bs-theme', 'light')
                break
        }
    }

    openChats() {
        this.router.navigate(['/chats'])
    }

    openRooms() {
        this.router.navigate(['/rooms'])
    }
}
