import { Injectable, signal, WritableSignal } from '@angular/core'
import { BaseApiService } from './base-api.service'
import { HttpClient } from '@angular/common/http'
import { ToastrService } from 'ngx-toastr'
import { environment } from '../../../environments/environment'
import { ImagesResponseDto } from '../models/dto'

const UPLOAD_IMAGES_URL = '/api/Auth/upload'
const GET_UPLOADED_IMAGES_URL = '/api/Auth/uploadedimages'

@Injectable({ providedIn: 'root' })
export class ImageService extends BaseApiService {

    private readonly apiUrl: string

    uploadedImages: WritableSignal<string[]> = signal([])

    constructor(http: HttpClient,
                alert: ToastrService) {
        super(http, alert)
        this.apiUrl = environment.apiUrl

        this.loadImages()
    }

    uploadImages(files: File[]) {
        const formData = new FormData()

        files.forEach((file) => {
            formData.append('documents', file, file.name)
        })

        return this.sendPost(this.apiUrl + UPLOAD_IMAGES_URL, formData)
            .then(() => this.loadImages())
    }


    loadImages() {
        this.sendGet<ImagesResponseDto>(this.apiUrl + GET_UPLOADED_IMAGES_URL)
            .then(data => {
                this.uploadedImages.set(data.Body.files.map(img => `${this.apiUrl}/${img}`))
            })
    }
}
