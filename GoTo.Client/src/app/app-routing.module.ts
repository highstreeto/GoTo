import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { WelcomeComponent } from './welcome/welcome.component';
import { TripAddComponent } from './trip-add/trip-add.component';
import { TripSearchComponent } from './trip-search/trip-search.component';

const routes: Routes = [
  { path: '', component: WelcomeComponent },
  { path: 'add-trip', component: TripAddComponent },
  { path: 'search-trip', component: TripSearchComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
