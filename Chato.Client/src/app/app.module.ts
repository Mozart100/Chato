import { BrowserModule } from '@angular/platform-browser'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import { NgModule } from '@angular/core'
import { HttpClientModule, HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http'
import { TranslateHttpLoader } from '@ngx-translate/http-loader'
import { TranslateModule, TranslateLoader } from '@ngx-translate/core'
import { ChatModule } from './chat/chat.module'
import { AppRoutingModule } from './app-routing.module'
import { AppComponent } from './app.component'
import { JwtInterceptor } from './core/helpers/jwt.interceptor'
import { NgbModule } from '@ng-bootstrap/ng-bootstrap'
import { ToastrModule } from 'ngx-toastr'


export function createTranslateLoader(http: HttpClient): any {
    return new TranslateHttpLoader(http, 'assets/i18n/', '.json')
}

@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        HttpClientModule,
        BrowserAnimationsModule,
        BrowserModule,
        AppRoutingModule,
        ChatModule,
        ToastrModule.forRoot(),
        TranslateModule.forRoot({
            defaultLanguage: 'en',
            loader: {
                provide: TranslateLoader,
                useFactory: createTranslateLoader,
                deps: [HttpClient]
            }
        }),
        NgbModule,
    ],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
