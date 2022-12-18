CREATE MIGRATION m15zazjc3ssohgkbhimiiwmowvkdqm2vtadl6ajj5erajgxslnmacq
    ONTO m1qxmbvsdehucrgmuyolanje2gvtedytbkmueyhtir7lb23qeffvma
{
  ALTER TYPE default::Photo {
      CREATE CONSTRAINT std::exclusive ON ((.name, .directory));
  };
};
