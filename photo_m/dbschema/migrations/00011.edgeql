CREATE MIGRATION m1skl3wqxuwr3dbopxlciqlu4emkoaxcjleyyngagbgsvbsnducfxq
    ONTO m15zazjc3ssohgkbhimiiwmowvkdqm2vtadl6ajj5erajgxslnmacq
{
  CREATE FUNCTION default::c_f_path(dir: std::str, n: std::str) ->  std::str USING (SELECT
      (dir ++ n)
  );
  DROP FUNCTION default::count_p_by_author(author_id: std::uuid);
};
