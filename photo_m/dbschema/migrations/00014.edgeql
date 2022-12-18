CREATE MIGRATION m1ziuts6fssum63zvoknmlb7g56mijcybz4kxg2tfkymmnugdpqatq
    ONTO m1hsbwk3427oo4qfo32jknlwmisllchpe4vstsg5vw3egexhqcnqqq
{
  ALTER TYPE default::Person {
      CREATE CONSTRAINT std::exclusive ON ((.name, .surname));
  };
};
