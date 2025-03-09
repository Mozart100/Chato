import { Component, OnInit, ViewChild } from '@angular/core'
import {
    NgbAccordionCollapse,
    NgbAccordionDirective,
    NgbAccordionHeader,
    NgbAccordionItem,
    NgbDropdown,
    NgbDropdownMenu, NgbDropdownToggle, NgbModal, NgbTooltip
} from '@ng-bootstrap/ng-bootstrap'
import { PickerComponent } from '@ctrl/ngx-emoji-mart'
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms'
import { SimplebarAngularModule } from 'simplebar-angular'
import { TranslateModule } from '@ngx-translate/core'
import { DatePipe, NgClass, NgIf } from '@angular/common'
import { Lightbox } from 'ngx-lightbox'
import { ChatStore } from '../../core/store/chat.store'
import { AuthenticationService } from '../../core/services/auth.service'
import { ChattoHubService } from '../../core/services/realtime.service'
import { IAlbum } from 'ngx-lightbox/lightbox-event.service'
import { SenderInfoType } from '../../core/models/chat.models'

@Component({
    selector: 'chat-window',
    standalone: true,
    imports: [
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
        SimplebarAngularModule,
        TranslateModule,
        NgClass,
        NgIf
    ],
    templateUrl: './chat-window.component.html',
    styleUrl: './chat-window.component.scss'
})
export class ChatWindowComponent implements OnInit {

    @ViewChild('scrollRef') scrollRef: any


    userProfile: any = 'assets/images/users/avatar-4.jpg'
    userName: any = 'Doris Brown'
    userStatus: any = 'online'

    message: any

    senderProfile: any

    isreplyMessage = false

    senderName: any

    formData!: FormGroup

    emoji = ''

    showEmojiPicker = false

    imageURL: string | undefined
    img: any

    constructor(private modalService: NgbModal,
                private lightbox: Lightbox,
                public formBuilder: FormBuilder,
                private datePipe: DatePipe,
                public chatStore: ChatStore,
                public auth: AuthenticationService,
                private realtime: ChattoHubService) {
    }

    ngOnInit() {
        this.formData = this.formBuilder.group({
            message: ['', [Validators.required]],
        })

        // const user = window.sessionStorage.getItem('currentUser')
        // if (user) {
        //     this.senderName = JSON.parse(user).username
        //     this.senderProfile = 'assets/images/users/' + JSON.parse(user).profile
        // } else {
        //     this.router.navigate(['/account/login'])
        // }
        // this.chat = chat
    }

    closeUserChat() {
        document.getElementById('chat-room').classList.remove('user-chat-show')
    }

    onChatInfoClicked() {
        // (document.querySelector('.user-profile-sidebar') as HTMLElement).style.display = 'block'
    }

    MessageSearch() {
        var input: any, filter: any, ul: any, li: any, a: any | undefined, i: any, txtValue: any
        input = document.getElementById('searchMessage') as HTMLAreaElement
        filter = input.value.toUpperCase()
        ul = document.getElementById('users-conversation')
        li = ul.getElementsByTagName('li')
        for (i = 0; i < li.length; i++) {
            a = li[i].getElementsByTagName('p')[0]
            txtValue = a?.innerText
            if (txtValue?.toUpperCase().indexOf(filter) > -1) {
                li[i].style.display = ''
            } else {
                li[i].style.display = 'none'
            }
        }
    }

    openCallModal(content) {
        this.modalService.open(content, { centered: true })
    }

    openVideoModal(videoContent) {
        this.modalService.open(videoContent, { centered: true })
    }

    showUserProfile() {
        document.getElementById('profile-detail').style.display = 'block'
    }

    // Delete All Message
    deleteAllMessage(event: any) {
        var allMsgDelete: any = document.getElementById('users-conversation')?.querySelectorAll('.chats')
        allMsgDelete.forEach((item: any) => {
            item.remove()
        })
    }

    /**
     * Open lightbox
     */
    openImage(src: string, idx: number): void {

        // create album with images
        const album: Array<IAlbum> = [{
            src: src,
            thumb: src
        }]

        // open lightbox
        this.lightbox.open(album, idx, {
            showZoom: true
        })

    }

    deleteMessage(event: any) {
        event.target.closest('.chats').remove()
    }

    replyMessage(event: any) {
        this.isreplyMessage = true
        document.querySelector('.replyCard')?.classList.add('show')
        var copyText = event.target.closest('.chats').querySelector('.messageText').innerHTML;
        (document.querySelector('.replyCard .replymessage-block .flex-grow-1 .mb-0') as HTMLAreaElement).innerHTML = copyText
        var msgOwnerName: any = event.target.closest('.chats').classList.contains('right') == true ? 'You' : document.querySelector('.username')?.innerHTML;
        (document.querySelector('.replyCard .replymessage-block .flex-grow-1 .conversation-name') as HTMLAreaElement).innerHTML = msgOwnerName
    }

    copyMessage(event: any) {
        navigator.clipboard.writeText(event.target.closest('.chats').querySelector('.messageText').innerHTML)
        document.getElementById('copyClipBoard')?.classList.add('show')
        setTimeout(() => {
            document.getElementById('copyClipBoard')?.classList.remove('show')
        }, 1000)
    }

    /**
     * Open center modal
     * @param centerDataModal center modal data
     */
    centerModal(centerDataModal: any) {
        this.modalService.open(centerDataModal, { centered: true })
    }

    /**
     * Save the message in chat
     */
    messageSave() {
        const text = this.formData.get('message')!.value as string
        if (text) {

            const message = {
                chatName: this.chatStore.selectedChat().roomName,
                fromUser: this.auth.user().userName,
                textMessage: text,
                isSelf: true,
                timeStemp: new Date().getTime(),
                senderInfoType: SenderInfoType.TextMessage
            }

            this.chatStore.selectedChat.update(chat => {
                if (!chat.messages) {
                    chat.messages = []
                }

                chat.messages.push(message)
                return chat
            })

            this.realtime.sendMessageToOthersInChat(message)

            this.onListScroll()

            // Set Form Data Reset
            this.formData = this.formBuilder.group({
                message: null,
            })
        }


        // var groupMsg = document.querySelector('.pills-groups-tab.active')
        // const message = this.formData.get('message')!.value
        // if (message) {
        //     if (!groupMsg) {
        //         document.querySelector('.chat-user-list li.active .chat-user-message').innerHTML = message ? message : this.img
        //     }
        //     var img = this.img ? this.img : ''
        //     var status = this.img ? true : ''
        //     var dateTime = this.datePipe.transform(new Date(), 'h:mm a')
        //     var chatReplyUser = this.isreplyMessage == true ? (document.querySelector('.replyCard .replymessage-block .flex-grow-1 .conversation-name') as HTMLAreaElement).innerHTML : ''
        //     var chatReplyMessage = this.isreplyMessage == true ? (document.querySelector('.replyCard .replymessage-block .flex-grow-1 .mb-0') as HTMLAreaElement).innerText : ''
        //     this.message.push({
        //         id: 1,
        //         message: message,
        //         name: this.senderName,
        //         profile: this.senderProfile,
        //         time: dateTime,
        //         align: 'right',
        //         isimage: status,
        //         imageContent: [img],
        //         replayName: chatReplyUser,
        //         replaymsg: chatReplyMessage,
        //     })
        //     this.onListScroll()
        //
        //     // Set Form Data Reset
        //     this.formData = this.formBuilder.group({
        //         message: null,
        //     })
        //     this.isreplyMessage = false
        //     this.emoji = ''
        //     this.img = ''
        //     chatReplyUser = ''
        //     chatReplyMessage = ''
        //     document.querySelector('.replyCard')?.classList.remove('show')
        // }
    }

    joinCurrentChat() {
        this.realtime.joinOnCreateChat(this.chatStore.selectedChat().roomName)
    }

    onBlur() {
    }

    onFocus() {
        this.showEmojiPicker = false
    }

    toggleEmojiPicker() {
        this.showEmojiPicker = !this.showEmojiPicker
    }

    addEmoji(event: any) {
        const { emoji } = this
        const text = `${emoji}${event.emoji.native}`
        this.emoji = text
        this.showEmojiPicker = false
    }

    fileChange(event: any) {
        let fileList: any = (event.target as HTMLInputElement)
        let file: File = fileList.files[0]
        const reader = new FileReader()
        reader.onload = () => {
            const imageURL = reader.result as string
            const bytesStr = imageURL.split(',')[1]
            // this.img = this.imageURL

            const message = {
                chatName: this.chatStore.selectedChat().roomName,
                fromUser: this.auth.user().userName,
                image: file.name,
                textMessage: bytesStr,
                isSelf: true,
                timeStemp: new Date().getTime(),
                senderInfoType: SenderInfoType.Image
            }

            this.chatStore.selectedChat().messages.push(message)

            this.realtime.sendMessageToOthersInChat(message)

        }
        reader.readAsDataURL(file)
    }


    onListScroll() {
        if (this.scrollRef !== undefined) {
            setTimeout(() => {
                this.scrollRef.SimpleBar.getScrollElement().scrollTop = this.scrollRef.SimpleBar.getScrollElement().scrollHeight
            }, 100)
        }
    }

    closeReplay() {
        document.querySelector('.replyCard')?.classList.remove('show')
    }

    CloseChatInfo() {
        (document.querySelector('.user-profile-sidebar') as HTMLElement).style.display = 'none'
    }
}
