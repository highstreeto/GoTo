/**
 * GoTo API
 * The GoTo REST API allows for offering trips and searching for public transport trips.
 *
 * OpenAPI spec version: v1
 * 
 *
 * NOTE: This class is auto generated by the swagger code generator program.
 * https://github.com/swagger-api/swagger-codegen.git
 * Do not edit the class manually.
 */


export interface TripOffer { 
    startLocation: string;
    starTime: Date;
    endTime: Date;
    endLocation: string;
    offeredBy: string;
}