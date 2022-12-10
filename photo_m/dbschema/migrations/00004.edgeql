CREATE MIGRATION m1himwh7hutowj2y5frzcdjrazbn7wwgvh25yskvc3kcchvq7ixhsq
    ONTO m12fgvubmfuwympybc3mfg5cnqhtbd6zz2znhmbzsihmmszc3rgc7q
{
  ALTER TYPE default::Human {
      DROP PROPERTY nick;
  };
};
