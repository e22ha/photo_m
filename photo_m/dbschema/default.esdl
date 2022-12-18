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
        multi link favorite_camera := (
            with p := Photographer
            select
                (group 
                    (select
                    Camera filter Camera.photographers = p)
                by .name)
            order by 
                (select count(Camera.photographers)) desc
            limit 1 
        );
        property nick -> str {constraint exclusive; };
        multi link photos := .<author[is Photo];

    }

    type Camera {
        required property brand -> str;
        required property model -> str;
        required property name := .brand ++ ' ' ++ .model;
        multi link photographers := .<camera[is Photographer];
    }


    type Photo {
        required property name -> str;
        required property directory -> str;
        required property full_path := .directory ++ .name;
        required link author -> Photographer;
        multi link face -> Person;
        property rating -> int64{
            default:=0;
        }
        link event -> Event;
    }

    type Event {
        required property title -> str;
        property date -> datetime;
    }

    function count_p_by_author(author_id: uuid)-> int64
        using(
            SELECT count(
            (
            select Photo
            filter .author.id = author_id
            )
            )
        )
}
