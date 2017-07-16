//
//  MapPin.h
//  Spotter2
//
//  Created by students@deti on 16/05/2017.
//  Copyright © 2017 students@deti. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <Mapkit/Mapkit.h>
@interface MapPin : NSObject<MKAnnotation>{
    NSString *title, *subtitle;
    CLLocationCoordinate2D coordinate;
    

}
@property(nonatomic,copy)NSString *title;
@property(nonatomic,copy)NSString *subtitle;
@property(nonatomic,assign) CLLocationCoordinate2D coordinate;

@end
