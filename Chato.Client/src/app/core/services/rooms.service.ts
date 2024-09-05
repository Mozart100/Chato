import { Injectable } from '@angular/core'
import { BaseApiService } from './base-api.service'
import { HttpClient } from '@angular/common/http'
import { ToastrService } from 'ngx-toastr'
import { environment } from '../../../environments/environment'
import { RoomsDto } from '../models/dto'
import { ChatStore } from '../store/chat.store'

const LOAD_ROOMS_URL = '/api/Room/all'

@Injectable({ providedIn: 'root' })
export class RoomsService extends BaseApiService {

    private readonly apiUrl: string

    constructor(http: HttpClient,
                alert: ToastrService,
                private chatStore: ChatStore) {
        super(http, alert)
        this.apiUrl = environment.apiUrl
    }

    loadAllRooms() {
        this.sendGet<RoomsDto>(this.apiUrl + LOAD_ROOMS_URL)
            .then(data => this.chatStore.allRooms.set(data.Body.rooms))
    }
}
