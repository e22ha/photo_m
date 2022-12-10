module default {
    abstract type Human {
        required property name -> str;
        property surname -> str;
        property full_name := .name ++ ' ' ++ .surname; 
    }

    type Person extending Human {
         
    }

    type Photographer extending Human{
        multi link camera -> Camera;
    }

    type Camera {
        required property brand -> str;
        required property model -> str;
        required property name := .brand ++ ' ' ++ .model;
    }

    scalar type Rate extending enum<None, Bad, NotBad, Normal, Super, Shdevr>;

    type Photo {
        required property name -> str;
        required property directory -> str;
        required property full_path := .directory ++ ' ' ++ .name;
        required link author -> Photographer;
        multi link face -> Person;
        property rating -> Rate{
            default:=Rate.None;
        }
        link event -> Event;
    }

    type Event {
        required property title -> str;
        property date -> datetime;
    }

}
