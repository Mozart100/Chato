import { Component, ElementRef, signal, ViewChild, WritableSignal } from '@angular/core'
import { TranslateModule, TranslateService } from '@ngx-translate/core'
import { NgbDropdown, NgbDropdownMenu, NgbDropdownToggle } from '@ng-bootstrap/ng-bootstrap'
import { ImageService } from '../../../core/services/image.service'
import { NgForOf } from '@angular/common'
import { IAlbum } from 'ngx-lightbox/lightbox-event.service'
import { Lightbox } from 'ngx-lightbox'
import { ToastrService } from 'ngx-toastr'
import { firstValueFrom } from 'rxjs'

@Component({
    selector: 'image-uploading-block',
    standalone: true,
    imports: [
        TranslateModule,
        NgbDropdown,
        NgbDropdownToggle,
        NgbDropdownMenu,
        NgForOf
    ],
    templateUrl: './image-uploading-block.component.html',
    styleUrl: './image-uploading-block.component.scss'
})
export class ImageUploadingBlockComponent {

    @ViewChild('fileInput', { static: true }) fileInput: ElementRef

    selectedFiles: WritableSignal<File[]> = signal([])
    private texts: any = {}

    constructor(public imageService: ImageService,
                private lightbox: Lightbox,
                private alert: ToastrService,
                private translate: TranslateService) {

        firstValueFrom(this.translate.get('imageUploadingBlock'))
            .then(texts => this.texts = texts)

    }

    onFileSelect(event: Event): void {
        const input = event.target as HTMLInputElement
        if (input.files) {
            this.selectedFiles.set(Array.from(input.files))
        }
    }

    uploadFiles(): void {
        if (this.selectedFiles().length > 0) {
            this.imageService.uploadImages(this.selectedFiles())
                .then(() => this.alert.success(this.texts['uploadSuccess']))
                .then(() => {
                    this.fileInput.nativeElement.value = ''
                    this.selectedFiles.set([])
                })
        }
    }

    openImage(idx: number): void {

        // create album with images
        const album: Array<IAlbum> = this.imageService.uploadedImages()
            .map(img => ({
                src: img,
                thumb: img,
            }))

        // open lightbox
        this.lightbox.open(album, idx, {
            showZoom: true
        })

    }
}
