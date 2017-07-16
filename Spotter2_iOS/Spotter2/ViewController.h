//
//  ViewController.h
//  Spotter2
//
//  Created by students@deti on 16/05/2017.
//  Copyright © 2017 students@deti. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <MapKit/MapKit.h>
#import "MapPin.h"
#import <AVFoundation/AVFoundation.h>

@interface ViewController : UIViewController <AVCaptureMetadataOutputObjectsDelegate>{
    IBOutlet MKMapView *mapview;

}
@property (weak, nonatomic) IBOutlet UIView *viewPreview;
@property (weak, nonatomic) IBOutlet UILabel *lblStatus;
@property (weak, nonatomic) IBOutlet UIBarButtonItem *bbitemStart;
- (IBAction)startStopReading:(id)sender;

@end
