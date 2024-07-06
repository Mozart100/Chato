import { Injectable, signal, WritableSignal } from '@angular/core'

@Injectable({ providedIn: 'root' })
export class NavStore {

    selectedTab: WritableSignal<number> = signal(1)

}
