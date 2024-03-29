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


/**
 * Parameters for a trip search
 */
export interface TripSearchParams { 
    /**
     * Name of the start location
     */
    startLocation: string;
    /**
     * Earliest trip start time
     */
    startTime: Date;
    /**
     * Name of the end location
     */
    endLocation: string;
}
