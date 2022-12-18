CREATE MIGRATION m1qxmbvsdehucrgmuyolanje2gvtedytbkmueyhtir7lb23qeffvma
    ONTO m1ktjjd42cdxlpxxs5kkq3wjkj3j2nybtxms45te3w2axqdvdpqq2q
{
  ALTER TYPE default::Camera {
      CREATE MULTI LINK photographers := (.<camera[IS default::Photographer]);
  };
  ALTER TYPE default::Photo {
      ALTER PROPERTY rating {
          SET default := 0;
          SET TYPE std::int64 USING (0);
      };
  };
  ALTER TYPE default::Photographer {
      CREATE MULTI LINK photos := (.<author[IS default::Photo]);
  };
  DROP SCALAR TYPE default::Rate;
};
