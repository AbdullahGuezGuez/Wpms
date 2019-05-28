export interface Activity {

    id: number;
    name: string;
    description: string;
    date: Date;
    type: string;
    archived: any;
    customerParticipants: any;
    businessParticipants: any;
    creatorId: number;

    //nextStep
    nextStepid: number;
    nextStepDescription: string;
    nextStepDate: Date;
    nextStepType: string;
    nextStepCustomerParticipants: any;
    nextStepBusinessParticipants: any;
    nextStepCreatorId: number;
}
