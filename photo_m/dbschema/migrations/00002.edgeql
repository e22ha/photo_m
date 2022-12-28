CREATE MIGRATION m1rdx3pu5oolp3fseas3w4zsccf42f4jvf7ig5ddv4ttps4komixzq
    ONTO m1gxx6hpwt32sbbdrhgfzjshnaw2wkqu7vbe2yfvkki6buaetqqzyq
{
  ALTER TYPE default::Person {
      CREATE MULTI LINK photos := (.<face[IS default::Photo]);
  };
};
