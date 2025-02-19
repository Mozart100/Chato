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
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms'
import { PickerComponent } from '@ctrl/ngx-emoji-mart'
import { ChatService } from '../../core/services/chat.service'
import { ChatStore } from '../../core/store/chat.store'
import { Chat } from '../../core/models/chat.models'
import { ChatWindowComponent } from '../chat-window/chat-window.component'
import { AuthenticationService } from '../../core/services/auth.service'
import { NgClass, NgIf } from '@angular/common'
import { ChattoHubService } from '../../core/services/realtime.service'
import { ToastrService } from 'ngx-toastr'

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
        NgClass,
        NgIf
    ],
    templateUrl: './rooms.component.html',
    styleUrl: './rooms.component.scss'
})
export class RoomsComponent implements OnInit {

    public isCollapsed = true

    createRoomForm: FormGroup


    constructor(private navStore: NavStore,
                private modalService: NgbModal,
                private roomsService: ChatService,
                public chatStore: ChatStore,
                private chatService: ChatService,
                private realtime: ChattoHubService,
                private fb: FormBuilder,
                private toastr: ToastrService) {

        this.navStore.selectedTab.set(3)

        this.createRoomForm = this.fb.group({
            groupName: ['', Validators.required],
            description: ['']
        })
    }

    ngOnInit() {
        this.roomsService.loadAllRooms()
    }

    openGroupModal(content: any) {
        this.modalService.open(content, { centered: true })
    }

    showGroupChat(room: Chat) {

        this.chatStore.selectedChat.set(room)

        if (!room.messages || room.messages.length == 0) {
            this.realtime.downloadHistory(this.chatStore.selectedChat())
        }

        document.querySelector('.user-chat').classList.add('user-chat-show')
    }

    GroupSearch() {
    }

    createGroup(modal: any) {
        const name = this.createRoomForm.get('groupName').value
        const description = this.createRoomForm.get('description').value

        this.realtime.createPublicGroup(name, description)
            .then(() => {
                modal.dismiss('Group Created')
                this.chatService.addNewChat(name)
            })
            .catch(err => this.toastr.error(err.error || err.message, 'Failed to create group'))
    }
}
