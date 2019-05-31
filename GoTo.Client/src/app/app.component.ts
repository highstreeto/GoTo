import { Component } from '@angular/core';
import { TripOfferService, TripOffer } from 'src/api-client';

@Component({
  selector: 'goto-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'GoTo-Client';
  offers: TripOffer[];

  constructor(private tripOfferService: TripOfferService) {
    // TODO Remove (just for testing the API client)
    this.tripOfferService.query()
      .forEach((offers: TripOffer[]) => {
        this.offers = offers;
      });
  }
}
