import { Component, OnInit } from '@angular/core'
import { NavStore } from '../../core/store/nav.store'
import { TranslateModule } from '@ngx-translate/core'
import {
    NgbAccordionCollapse,
    NgbAccordionDirective,
    NgbAccordionHeader,
    NgbAccordionItem,
    NgbCollapse, NgbDropdown, NgbDropdownMenu, NgbDropdownToggle,
    NgbModal, NgbTooltip
} from '@ng-bootstrap/ng-bootstrap'
import { groups } from '../index/data'
import { SimplebarAngularModule } from 'simplebar-angular'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { PickerComponent } from '@ctrl/ngx-emoji-mart'
import { ChatService } from '../../core/services/chat.service'
import { ChatStore } from '../../core/store/chat.store'
import { Chat } from '../../core/models/chat.models'
import { ChatWindowComponent } from '../chat-window/chat-window.component'
import { AuthenticationService } from '../../core/services/auth.service'
import { NgClass } from '@angular/common'

@Component({
    selector: 'app-rooms',
    standalone: true,
    imports: [
        TranslateModule,
        SimplebarAngularModule,
        NgbCollapse,
        FormsModule,
        NgbAccordionCollapse,
        NgbAccordionDirective,
        NgbAccordionHeader,
        NgbAccordionItem,
        NgbDropdown,
        NgbDropdownMenu,
        NgbDropdownToggle,
        NgbTooltip,
        PickerComponent,
        ReactiveFormsModule,
        ChatWindowComponent,
        NgClass
    ],
    templateUrl: './rooms.component.html',
    styleUrl: './rooms.component.scss'
})
export class RoomsComponent implements OnInit {

    public isCollapsed = true

    constructor(private navStore: NavStore,
                private modalService: NgbModal,
                private roomsService: ChatService,
                public chatStore: ChatStore,
                public authService: AuthenticationService) {
        this.navStore.selectedTab.set(3)
    }

    ngOnInit() {
        this.roomsService.loadAllRooms()
    }

    openGroupModal(content: any) {
        this.modalService.open(content, { centered: true })
    }

    showGroupChat(room: Chat) {

        this.chatStore.selectedRoom.set(room)

        // document.querySelector('.user-chat').classList.add('user-chat-show')
        // document.querySelector('.chat-welcome-section').classList.add('d-none')
        // document.querySelector('.user-chat').classList.remove('d-none')
        // event.target.closest('li').classList.add('active')
        // var data = this.groups.filter((group: any) => {
        //     return group.id === id
        // })
        // this.userName = data[0].name
        // this.userProfile = ''
        // this.message = data[0].messages
    }

    GroupSearch() {
        /*        var input: any, filter: any, ul: any, li: any, a: any | undefined, i: any, txtValue: any
                input = document.getElementById('searchGroup') as HTMLAreaElement
                filter = input.value.toUpperCase()
                ul = document.querySelectorAll('.group-list')
                ul.forEach((item: any) => {
                    li = item.getElementsByTagName('li')
                    for (i = 0; i < li.length; i++) {
                        a = li[i].getElementsByTagName('h5')[0]
                        txtValue = a?.innerText
                        if (txtValue?.toUpperCase().indexOf(filter) > -1) {
                            li[i].style.display = ''
                        } else {
                            li[i].style.display = 'none'
                        }
                    }
                })*/
    }
}
