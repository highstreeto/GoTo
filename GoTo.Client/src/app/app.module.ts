import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { ApiModule, Configuration, ConfigurationParameters } from 'src/api-client';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { WelcomeComponent } from './welcome/welcome.component';
import { TripSearchComponent } from './trip-search/trip-search.component';
import { TripAddComponent } from './trip-add/trip-add.component';

@NgModule({
  declarations: [
    AppComponent,
    WelcomeComponent,
    TripSearchComponent,
    TripAddComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    NgbModule,
    ApiModule.forRoot(apiConfigFactory)
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

export function apiConfigFactory(): Configuration {
  const params: ConfigurationParameters = {
    basePath: 'http://localhost:5000'
  };
  return new Configuration(params);
}
